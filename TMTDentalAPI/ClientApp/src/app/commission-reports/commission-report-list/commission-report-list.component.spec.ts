import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionReportListComponent } from './commission-report-list.component';

describe('CommissionReportListComponent', () => {
  let component: CommissionReportListComponent;
  let fixture: ComponentFixture<CommissionReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
