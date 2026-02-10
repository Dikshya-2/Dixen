import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GenericService } from '../../Services/generic-service';
import { EventSummary } from '../../Models/EventSummary';
import { HttpClient } from '@angular/common/http';
import { AgCharts } from "ag-charts-angular";
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
// import * as agCharts from 'ag-charts-community';  
import { ModuleRegistry, AllCommunityModule } from 'ag-charts-community';  


@Component({
  selector: 'app-analysis',
  standalone: true,
  imports: [CommonModule, AgCharts,RouterModule, FormsModule ],
  providers: [GenericService],
  templateUrl: './analysis.html',
  styleUrl: './analysis.css'
})
export class Analysis implements OnInit {
events: EventSummary[] = [];
  totalTickets = 0;
  totalShares = 0;
  chartOptions: any;

  constructor(
    private genericService: GenericService<EventSummary>,
    private httpClient: HttpClient
  ) {
    ModuleRegistry.registerModules([AllCommunityModule]);
  }

  ngOnInit(): void {
    this.directApiCall();
  }

  trackByEventId(index: number, event: EventSummary): number {
    return event.eventId;
  }

  directApiCall() {
    this.httpClient
      .get<any[]>('https://localhost:7204/api/reporting/events-summary')
      .subscribe({
        next: (data) => {
          console.log('Data loaded:', data);
          this.events = data;
          this.totalTickets = data.reduce((sum, e) => sum + (e.ticketsSold ?? 0), 0);
          this.totalShares = data.reduce((sum, e) => sum + (e.sharesCount ?? 0), 0);

          this.chartOptions = {
            title: { 
              text: 'Social Engagement',
              fontSize: 16,
              color: '#1e293b'
            },
            subtitle: {
              text: `${this.totalShares.toLocaleString()} total shares`,
              fontSize: 14,
              color: '#64748b'
            },
            data: data
              .filter(e => (e.sharesCount ?? 0) > 0)
              .slice(0, 8)
              .map((e: any) => ({
                event: e.title?.length! > 25 ? e.title!.substring(0, 25) + '...' : e.title!,
                shares: e.sharesCount ?? 0,
                tickets: e.ticketsSold ?? 0
              })),
            series: [
              {
                type: 'bar',
                xKey: 'event',
                yKey: 'shares',
                yName: 'Shares',
                fill: '#10b981',
                fillOpacity: 0.9,
                strokeWidth: 0
              },
              {
                type: 'bar',
                xKey: 'event',
                yKey: 'tickets',
                yName: 'Tickets',
                fill: '#3b82f6',
                fillOpacity: 0.85,
                strokeWidth: 0
              }
            ],
            axes: {
              bottom: {
                type: 'category',
                title: { text: 'Events' },
                label: { rotation: 45 }
              },
              left: {
                type: 'number',
                title: { text: 'Count' },
              }
            },
            legend: { position: 'top' }
          };

          console.log('Chart Options:', this.chartOptions);
        },
        error: (err) => console.error('API Error:', err)
      });
  }
}