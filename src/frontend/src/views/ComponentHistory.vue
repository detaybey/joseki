<template>
  <div>
    <Spinner v-if="!loaded" :loadFailed="loadFailed" @reload="loadData" />
    <div v-if="loaded" class="segment shadow" style="min-height:300px;background-color:#eee;">  
      <div style="display:flex;flex-direction:column;width:100%;">
        <!-- <div class='subscription-history-header'>
          <h1>
            {{ component.category }}  
            <b v-if="component.category !== 'Overall'"> : {{ decodeURIComponent(component.id) }}</b>
          </h1>          
          <h2 v-show="component.category !== 'Overall'">Name : <b>{{ decodeURIComponent(component.name) }}</b></h2>
        </div> -->
        <div class="chart-container">
          <apexchart height="150" :options="getAreaChartOptions()" :series="getAreaSeries()"></apexchart>
        </div>
        <div class='table-container'>
          <table>
            <thead>
                <tr>
                    <td colspan="2">Scan Date</td>
                    <td class='text-right'>
                      No Data 
                      <span class='text-center meta-tooltip' data-balloon-length="xlarge" data-balloon-pos="up" 
                          :aria-label="meta('metadata.checks_nodata_description')"><span class="icon-help-circle"></span></span>
                    </td>
                    <td class='text-right'>
                      Warning
                       <span class='text-center meta-tooltip' data-balloon-length="xlarge" data-balloon-pos="up"
                       :aria-label="meta('metadata.checks_warning_description')"><span class="icon-help-circle"></span></span>
                    </td>
                    <td class='text-right'>                      
                      Failed
                      <span class='text-center meta-tooltip' data-balloon-length="xlarge" data-balloon-pos="up" 
                      :aria-label="meta('metadata.checks_failed_description')"><span class="icon-help-circle "></span></span>
                      </td>
                    <td class='text-right'>
                      Passed
                      <span class='text-center meta-tooltip' data-balloon-length="xlarge" data-balloon-pos="up" 
                      :aria-label="meta('metadata.checks_passed_description')"><span class="icon-help-circle"></span></span>
                      </td>
                    <td class='text-right'>Score
                      <span class='text-center meta-tooltip' data-balloon-length="xlarge" data-balloon-pos="up-right" 
                      :aria-label="meta('metadata.checks_score_description')"><span class="icon-help-circle"></span></span>
                    </td>
                </tr>
            </thead>
            <tbody>
            <tr v-for="(scan,i) in data" :key="`scan${i}1`" class="zigzag">
              <td>
                <label :for="`scan${i}1`">
                <input type="checkbox" class='chk' :id="`scan${i}1`" :value="`${scan.date}`" v-model="checkedScans" 
                :disabled="checkDisabled(i, `${scan.date}`)">
                <span class="pl-2">{{ scan.date | formatDate }}</span>
                </label>
              </td>
              <td style="font-size:11px;" class='external-link'>
                <a class='show-on-hover' :href="getscanDetailurl(scan.date)" :target="`out_${scan.date}`"><i class="icon-arrow-up-right"></i>scan detail</a>
              </td>
              <td class='text-sm text-right'>{{ scan.current.noData || '0' }}</td>
              <td class='text-sm text-right'>{{ scan.current.warning || '0' }}</td>
              <td class='text-sm text-right'>{{ scan.current.failed || '0' }}</td>
              <td class='text-sm text-right'>{{ scan.current.passed || '0' }}</td>
              <td class="text-sm text-right">{{ scan.current.score }}%</td>
            </tr> 
            </tbody>          
          </table>
        </div>
        <div class="subscription-history-buttons">
            <button class="btn mr-4" @click="CompareScans()" :disabled='canCompare()'>
              <span class="icon-copy pr-1"></span>Compare
            </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" src="./ComponentHistory.ts"></script>
<style lang="scss" scoped src="./ComponentHistory.scss"></style>


