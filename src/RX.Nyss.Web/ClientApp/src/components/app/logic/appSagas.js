import { call, put, takeEvery, select } from "redux-saga/effects";
import * as consts from "./appConstans";
import * as actions from "./appActions";
import { updateStrings } from "../../../strings";
import * as http from "../../../utils/http";
import {  removeAccessToken, isAccessTokenSet } from "../../../authentication/auth";
import { push } from "connected-react-router";
import { getBreadcrumb, getMenu, placeholders } from "../../../siteMap";

export const appSagas = () => [
  takeEvery(consts.INIT_APPLICATION.INVOKE, initApplication),
  takeEvery(consts.OPEN_MODULE.INVOKE, openModule),
];

function* initApplication() {
  yield put(actions.initApplication.request());
  try {
    const user = yield call(getAndVerifyUser);

    if (user) {
      yield call(getStrings);
      yield call(getAppData);
    }

    yield put(actions.initApplication.success());
  } catch (error) {
    yield put(actions.initApplication.failure(error.message));
  }
};

function* openModule({ path, params }) {
  const breadcrumb = getBreadcrumb(path, params);
  const topMenu = getMenu("/", params, placeholders.topMenu, path);
  const sideMenu = getMenu(path, params, placeholders.leftMenu, path);

  yield put(actions.openModule.success(path, params, breadcrumb, topMenu, sideMenu))
}

function* reloadPage() {
  const pathname = yield select(state => state.router.location.pathname);
  yield put(push(pathname));
}

function* getAndVerifyUser() {
  if (!isAccessTokenSet()) {
    return null;
  }

  const user = yield call(getUserStatus);

  if (!user) {
    removeAccessToken();
    yield reloadPage();
    return null;
  }

  return user;
};

function* getUserStatus() {
  yield put(actions.getUser.request());
  try {
    const status = yield call(http.get, "/api/authentication/status");

    const user = status.value.isAuthenticated
      ? { name: status.value.data.name, roles: status.value.data.roles }
      : null;

    yield put(actions.getUser.success(status.value.isAuthenticated, user));
    return user;
  } catch (error) {
    yield put(actions.getUser.failure(error.message));
  }
};

function* getAppData() {
  yield put(actions.getAppData.request());
  try {
    const appData = yield call(http.get, "/api/appData/get");
    yield put(actions.getAppData.success(appData.value.contentLanguages, appData.value.countries));
  } catch (error) {
    yield put(actions.getAppData.failure(error.message));
  }
};

function* getStrings() {
  yield put(actions.getStrings.invoke());
  try {
    // api call
    updateStrings({});
    yield put(actions.getStrings.success());
  } catch (error) {
    yield put(actions.getStrings.failure(error.message));
  }
};