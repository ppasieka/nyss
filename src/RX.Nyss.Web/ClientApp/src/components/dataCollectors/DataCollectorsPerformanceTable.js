import styles from './DataCollectorsPerformanceTable.module.scss';
import React, { useState } from 'react';
import PropTypes from "prop-types";
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import { strings, stringKeys } from '../../strings';
import { TableContainer } from '../common/table/TableContainer';
import { getIconFromStatus } from './logic/dataCollectorsService';
import { DataCollectorStatusIcon } from '../common/icon/DataCollectorStatusIcon';
import Icon from '@material-ui/core/Icon';
import { DataCollectorsPerformanceColumnFilters } from './DataCollectorsPerformanceColumnFilters';
import TablePager from '../common/tablePagination/TablePager';

export const DataCollectorsPerformanceTable = ({ list, page, rowsPerPage, totalRows, isListFetching, filters, onChange }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState(null);
  const [selectedWeek, setSelectedWeek] = useState(null);
  const [statusFilters, setStatusFilters] = useState(null);

  const openFilter = (event) => {
    setAnchorEl(event.currentTarget);
    setSelectedWeek(event.currentTarget.id);
    setStatusFilters(getStatusFilter(event.currentTarget.id));
    setIsOpen(true);
  }

  const getStatusFilter = (status) => {
    switch (status) {
      case 'lastWeek': return Object.assign({}, filters.lastWeek);
      case 'twoWeeksAgo': return Object.assign({}, filters.twoWeeksAgo);
      case 'threeWeeksAgo': return Object.assign({}, filters.threeWeeksAgo);
      case 'fourWeeksAgo': return Object.assign({}, filters.fourWeeksAgo);
      case 'fiveWeeksAgo': return Object.assign({}, filters.fiveWeeksAgo);
      case 'sixWeeksAgo': return Object.assign({}, filters.sixWeeksAgo);
      case 'sevenWeeksAgo': return Object.assign({}, filters.sevenWeeksAgo);
      case 'eightWeeksAgo': return Object.assign({}, filters.eightWeeksAgo);
      default: return null;
    }
  }

  const filterIsActive = (status) => {
    switch (status) {
      case 'lastWeek': return !filters.lastWeek.reportingCorrectly || !filters.lastWeek.reportingWithErrors || !filters.lastWeek.notReporting;
      case 'twoWeeksAgo': return !filters.twoWeeksAgo.reportingCorrectly || !filters.twoWeeksAgo.reportingWithErrors || !filters.twoWeeksAgo.notReporting;
      case 'threeWeeksAgo': return !filters.threeWeeksAgo.reportingCorrectly || !filters.threeWeeksAgo.reportingWithErrors || !filters.threeWeeksAgo.notReporting;
      case 'fourWeeksAgo': return !filters.fourWeeksAgo.reportingCorrectly || !filters.fourWeeksAgo.reportingWithErrors || !filters.fourWeeksAgo.notReporting;
      case 'fiveWeeksAgo': return !filters.fiveWeeksAgo.reportingCorrectly || !filters.fiveWeeksAgo.reportingWithErrors || !filters.fiveWeeksAgo.notReporting;
      case 'sixWeeksAgo': return !filters.sixWeeksAgo.reportingCorrectly || !filters.sixWeeksAgo.reportingWithErrors || !filters.sixWeeksAgo.notReporting;
      case 'sevenWeeksAgo': return !filters.sevenWeeksAgo.reportingCorrectly || !filters.sevenWeeksAgo.reportingWithErrors || !filters.sevenWeeksAgo.notReporting;
      case 'eightWeeksAgo': return !filters.eightWeeksAgo.reportingCorrectly || !filters.eightWeeksAgo.reportingWithErrors || !filters.eightWeeksAgo.notReporting;
      default: return false;
    }
  }

  const onChangePage = (e, page) => {
    onChange({ type: 'changePage', pageNumber: page });
  }

  const handleClose = (fields) => {
    onChange({ type: 'updateSorting', week: selectedWeek, filters: fields });
    setIsOpen(false);
  }

  return !!filters && (
    <TableContainer sticky isFetching={isListFetching}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>{strings(stringKeys.dataCollector.performanceList.name)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.performanceList.daysSinceLastReport)}</TableCell>
            <TableCell>
              <div id="lastWeek" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusLastWeek)}
                <Icon className={styles.filterIcon}>{filterIsActive('lastWeek') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>

            </TableCell>
            <TableCell>
              <div id="twoWeeksAgo" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusTwoWeeksAgo)}
                <Icon className={styles.filterIcon}>{filterIsActive('twoWeeksAgo') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>
            </TableCell>
            <TableCell>
              <div id="threeWeeksAgo" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusThreeWeeksAgo)}
                <Icon className={styles.filterIcon}>{filterIsActive('threeWeeksAgo') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>
            </TableCell>
            <TableCell>
              <div id="fourWeeksAgo" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusFourWeeksAgo)}
                <Icon className={styles.filterIcon}>{filterIsActive('fourWeeksAgo') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>
            </TableCell>
            <TableCell>
              <div id="fiveWeeksAgo" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusFiveWeeksAgo)}
                <Icon className={styles.filterIcon}>{filterIsActive('fiveWeeksAgo') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>
            </TableCell>
            <TableCell>
              <div id="sixWeeksAgo" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusSixWeeksAgo)}
                <Icon className={styles.filterIcon}>{filterIsActive('sixWeeksAgo') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>
            </TableCell>
            <TableCell>
              <div id="sevenWeeksAgo" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusSevenWeeksAgo)}
                <Icon className={styles.filterIcon}>{filterIsActive('sevenWeeksAgo') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>
            </TableCell>
            <TableCell>
              <div id="eightWeeksAgo" onClick={openFilter} className={styles.filterHeader}>
                {strings(stringKeys.dataCollector.performanceList.statusEightWeeksAgo)}
                <Icon className={styles.filterIcon}>{filterIsActive('eightWeeksAgo') ? 'filter_alt' : 'expand_more'}</Icon>
              </div>
            </TableCell>
            <TableCell />
          </TableRow>
        </TableHead>
        <TableBody>
          {!isListFetching && (
            list.map((row, index) => (
              <TableRow key={index} hover>
                <TableCell>{row.name}</TableCell>
                <TableCell style={{ textAlign: "center" }}>{row.daysSinceLastReport > -1 ? row.daysSinceLastReport : '-'}</TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusLastWeek} icon={getIconFromStatus(row.statusLastWeek)} />
                </TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusTwoWeeksAgo} icon={getIconFromStatus(row.statusTwoWeeksAgo)} />
                </TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusThreeWeeksAgo} icon={getIconFromStatus(row.statusThreeWeeksAgo)} />
                </TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusFourWeeksAgo} icon={getIconFromStatus(row.statusFourWeeksAgo)} />
                </TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusFiveWeeksAgo} icon={getIconFromStatus(row.statusFiveWeeksAgo)} />
                </TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusSixWeeksAgo} icon={getIconFromStatus(row.statusSixWeeksAgo)} />
                </TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusSevenWeeksAgo} icon={getIconFromStatus(row.statusSevenWeeksAgo)} />
                </TableCell>
                <TableCell style={{ textAlign: "center" }}>
                  <DataCollectorStatusIcon status={row.statusEightWeeksAgo} icon={getIconFromStatus(row.statusEightWeeksAgo)} />
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
      {!!list.length && <TablePager totalRows={totalRows} rowsPerPage={rowsPerPage} page={page} onChangePage={onChangePage} />}

      <DataCollectorsPerformanceColumnFilters
        open={isOpen}
        anchorEl={anchorEl}
        filters={statusFilters}
        onClose={handleClose} />
    </TableContainer>
  );
}

DataCollectorsPerformanceTable.propTypes = {
  isListFetching: PropTypes.bool,
  list: PropTypes.array,
  filters: PropTypes.object,
  getDataCollectorPerformanceList: PropTypes.func
};

export default DataCollectorsPerformanceTable;
