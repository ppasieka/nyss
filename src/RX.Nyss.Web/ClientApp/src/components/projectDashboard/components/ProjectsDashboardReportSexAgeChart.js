import React from 'react';
import { Card, CardContent, CardHeader } from '@material-ui/core';
import Highcharts from 'highcharts'
import HighchartsReact from 'highcharts-react-official'
import { strings, stringKeys } from '../../../strings';

const getOptions = (valuesLabel, series, categories) => ({
  chart: {
    type: 'column',
    backgroundColor: "transparent",
    style: {
      fontFamily: 'Poppins,"Helvetica Neue",Arial'
    }
  },
  title: {
    text: ''
  },
  xAxis: {
    categories: categories,
  },
  yAxis: {
    title: {
      text: valuesLabel
    },
    allowDecimals: false
  },
  legend: {
    enabled: true,
    itemStyle: { fontWeight: "regular" }
  },
  credits: {
    enabled: false
  },
  plotOptions: {
    column: {
      stacking: 'normal'
    }
  },
  tooltip: {
    headerFormat: '',
    pointFormat: '{series.name}: <b>{point.y}</b>'
  },
  series
});

export const ProjectsDashboardReportSexAgeChart = ({ data }) => {
  const categories = data.map(d => d.period);

  const series = [
    {
      name: strings(stringKeys.dashboard.reportsPerFeatureAndDate.femalesAbove5, true),
      data: data.map(d => d.countFemalesAtLeastFive),
      color: "#078e5e"
    },
    {
      name: strings(stringKeys.dashboard.reportsPerFeatureAndDate.femalesBelow5, true),
      data: data.map(d => d.countFemalesBelowFive),
      color: "#47c79a"
    },
    {
      name: strings(stringKeys.dashboard.reportsPerFeatureAndDate.malesAbove5, true),
      data: data.map(d => d.countMalesAtLeastFive),
      color: "#00a0dc"
    },
    {
      name: strings(stringKeys.dashboard.reportsPerFeatureAndDate.malesBelow5, true),
      data: data.map(d => d.countMalesBelowFive),
      color: "#72d5fb"
    },
    {
      name: strings(stringKeys.dashboard.reportsPerFeatureAndDate.unspecifiedSexAndAge, true),
      data: data.map(d => d.countUnspecifiedSexAndAge),
      color: "#c2b5ce"
    }
  ];

  const chartData = getOptions(strings(stringKeys.dashboard.reportsPerFeatureAndDate.numberOfReports, true), series, categories);

  return (
    <Card data-printable={true}>
      <CardHeader title={strings(stringKeys.dashboard.reportsPerFeatureAndDate.title)} />
      <CardContent>
        <HighchartsReact
          highcharts={Highcharts}
          ref={element => element && element.chart.reflow()}
          options={chartData}
        />
      </CardContent>
    </Card>
  );
}
