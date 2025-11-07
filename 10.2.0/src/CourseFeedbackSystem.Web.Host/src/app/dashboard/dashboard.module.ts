import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        SharedModule,
        DashboardRoutingModule,
        CommonModule,
        DashboardComponent,
    ],
})
export class DashboardModule {}

