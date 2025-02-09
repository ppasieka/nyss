import { Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer, Supervisor, HeadSupervisor, Coordinator } from "./roles";

export const accessMap = {
  nationalSocieties: {
    showDashboard: [Administrator, GlobalCoordinator, Manager, Coordinator, TechnicalAdvisor, DataConsumer],
    list: [Administrator, GlobalCoordinator, TechnicalAdvisor, DataConsumer],
    add: [Administrator, GlobalCoordinator],
    edit: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, Coordinator],
    delete: [Administrator, GlobalCoordinator],
    archive: [Administrator, GlobalCoordinator],
    get: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer, Coordinator],
  },
  nationalSocietyStructure: {
    edit: [Administrator, Manager, TechnicalAdvisor, Coordinator]
  },
  smsGateways: {
    list: [Administrator, Manager, TechnicalAdvisor, Coordinator],
    add: [Administrator, Manager, TechnicalAdvisor, Coordinator],
    edit: [Administrator, Manager, TechnicalAdvisor, Coordinator],
    delete: [Administrator, Manager, TechnicalAdvisor, Coordinator]
  },
  eidsrIntegration: {
    get: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer, Coordinator],
    edit: [Administrator, Manager, TechnicalAdvisor, Coordinator],
  },
  organizations: {
    list: [Administrator, Coordinator, Manager, TechnicalAdvisor],
    add: [Administrator, Coordinator, Manager, TechnicalAdvisor],
    edit: [Administrator, Coordinator, Manager, TechnicalAdvisor],
    delete: [Administrator, Coordinator, Manager, TechnicalAdvisor]
  },
  projects: {
    get: [Administrator, Manager, TechnicalAdvisor, DataConsumer, Supervisor, HeadSupervisor, Coordinator],
    showOverview: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor, Coordinator],
    showDashboard: [Administrator, Manager, TechnicalAdvisor, DataConsumer, Supervisor, HeadSupervisor, Coordinator],
    list: [Administrator, Manager, TechnicalAdvisor, DataConsumer, Coordinator],
    add: [Administrator, Manager, TechnicalAdvisor, Coordinator],
    edit: [Administrator, Manager, TechnicalAdvisor, Coordinator],
    delete: [Administrator, Manager, TechnicalAdvisor, Coordinator]
  },
  nationalSocietyUsers: {
    list: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, Coordinator],
    add: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, Coordinator],
    edit: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, Coordinator],
    delete: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, Coordinator],
    headManagerAccess: [GlobalCoordinator, Administrator, Coordinator]
  },
  globalCoordinators: {
    list: [Administrator, GlobalCoordinator],
    add: [Administrator],
    edit: [Administrator],
    delete: [Administrator]
  },
  healthRisks: {
    list: [Administrator, GlobalCoordinator],
    add: [Administrator, GlobalCoordinator],
    edit: [Administrator, GlobalCoordinator],
    delete: [Administrator, GlobalCoordinator]
  },
  suspectedDiseases: {
    list: [Administrator, GlobalCoordinator],
    add: [Administrator, GlobalCoordinator],
    edit: [Administrator, GlobalCoordinator],
    delete: [Administrator, GlobalCoordinator]
  },
  dataCollectors: {
    list: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    performanceList: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    add: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    edit: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    delete: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    export: [Administrator, Manager, TechnicalAdvisor, HeadSupervisor, Supervisor],
    replaceSupervisor: [Administrator, Manager, TechnicalAdvisor]
  },
  projectOrganizations: {
    list: [Administrator, Coordinator, Manager, TechnicalAdvisor],
    add: [Administrator, Coordinator],
    delete: [Administrator, Coordinator]
  },
  projectAlertNotifications: {
    list: [Administrator, Manager, TechnicalAdvisor],
    addRecipient: [Administrator, Manager, TechnicalAdvisor],
    editRecipient: [Administrator, Manager, TechnicalAdvisor],
    deleteRecipient: [Administrator, Manager, TechnicalAdvisor]
  },
  reports: {
    list: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    edit: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    sendReport: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    goToAlert: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    crossCheck: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor]
  },
  nationalSocietyReports: {
    list: [Administrator, Manager, TechnicalAdvisor],
  },
  alerts: {
    list: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor, Coordinator],
    assess: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor, Coordinator],
  },
  alertEvents: {
    list: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    add: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    edit: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
    delete: [Administrator, Manager, TechnicalAdvisor, Supervisor, HeadSupervisor],
  },
  translations: {
    list: [Administrator, GlobalCoordinator]
  },
  projectErrorMessages: {
    list: [Administrator, Manager, TechnicalAdvisor],
    edit: [Administrator, Manager, TechnicalAdvisor],
  }
};
