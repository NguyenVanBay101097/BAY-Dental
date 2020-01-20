import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenueReportManagerComponent } from './revenue-report-manager.component';

describe('RevenueReportManagerComponent', () => {
  let component: RevenueReportManagerComponent;
  let fixture: ComponentFixture<RevenueReportManagerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenueReportManagerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenueReportManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
