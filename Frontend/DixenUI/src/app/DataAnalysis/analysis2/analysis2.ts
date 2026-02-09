import { Component } from '@angular/core';
import { GenericService } from '../../Services/generic-service';
import { AgCharts } from "ag-charts-angular";
import { CommonModule } from '@angular/common';
import { AllCommunityModule, ModuleRegistry } from 'ag-charts-community';  


@Component({
  selector: 'app-analysis2',
  imports: [CommonModule, AgCharts], 
  templateUrl: './analysis2.html',
  styleUrl: './analysis2.css',
})
export class Analysis2 {
chartOptions: any;
  constructor(private genericService: GenericService<any>) {
    ModuleRegistry.registerModules([AllCommunityModule]); 
  }
  ngOnInit(): void { 
    this.genericService.getAll('/SocialShare/stats/event-platforms')
      .subscribe(data => {
        console.log('Social Share Data:', data);
        const totalCount = data.reduce((sum: number, d: any) => sum + d.count, 0);
        this.chartOptions = {
          title: { text: 'Social Shares by Platform' },
          data,
          series: [{
            type: 'donut',
            angleKey: 'count',
            sectorLabelKey: 'platform',
            innerRadiusRatio: 0.5,
            sectorLabel: {
              formatter: (params: any) => {
                const percent = ((params.datum.count / totalCount) * 100).toFixed(1);
                return `${params.datum.platform}: ${percent}%`;
              }
            }
          }]
        };
        console.log('Chart Options:', this.chartOptions);
      });
  }
}
