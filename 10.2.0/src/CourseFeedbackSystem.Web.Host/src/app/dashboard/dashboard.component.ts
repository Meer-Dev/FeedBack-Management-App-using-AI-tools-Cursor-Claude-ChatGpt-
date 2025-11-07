import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { DashboardServiceProxy, DashboardStatisticsDto, TopCoursesDto } from '@shared/service-proxies/service-proxies';
import { CommonModule } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

@Component({
    templateUrl: './dashboard.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [CommonModule, LocalizePipe],
})
export class DashboardComponent extends AppComponentBase implements OnInit {
    statistics: DashboardStatisticsDto = new DashboardStatisticsDto();
    topCourses: TopCoursesDto[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private _dashboardService: DashboardServiceProxy,
        private cd: ChangeDetectorRef  // ✅ ADD THIS
    ) {
        super(injector);
    }

    ngOnInit(): void {
        // ✅ Initialize statistics with default values
        this.statistics = {
            totalFeedbacks: 0,
            totalCourses: 0,
            activeCourses: 0,
            averageRating: 0
        } as DashboardStatisticsDto;
        
        this.loadStatistics();
        this.loadTopCourses();
    }

    loadStatistics(): void {
        this.loading = true;
        
        this._dashboardService.getStatistics().subscribe({
            next: (result: DashboardStatisticsDto) => {
                // ✅ Wrap in Promise to avoid change detection errors
                Promise.resolve().then(() => {
                    this.statistics = result;
                    this.loading = false;
                    this.cd.detectChanges();
                    console.log('Statistics loaded:', this.statistics);
                });
            },
            error: (err) => {
                console.error('Error loading statistics:', err);
                this.loading = false;
                abp.notify.error(this.l('AnErrorOccurred'));
                this.cd.detectChanges();
            }
        });
    }

    loadTopCourses(): void {
        this._dashboardService.getTopCoursesByRating(5).subscribe({
            next: (result: TopCoursesDto[]) => {
                // ✅ Wrap in Promise to avoid change detection errors
                Promise.resolve().then(() => {
                    this.topCourses = result || [];
                    this.cd.detectChanges();
                    console.log('Top courses loaded:', this.topCourses);
                });
            },
            error: (err) => {
                console.error('Error loading top courses:', err);
                abp.notify.error(this.l('AnErrorOccurred'));
                this.cd.detectChanges();
            }
        });
    }
}