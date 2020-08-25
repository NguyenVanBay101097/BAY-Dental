import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionReportDetailComponent } from './commission-report-detail.component';

describe('CommissionReportDetailComponent', () => {
  let component: CommissionReportDetailComponent;
  let fixture: ComponentFixture<CommissionReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
