import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerAdvanceDetailListReportComponent } from './partner-advance-detail-list-report.component';

describe('PartnerAdvanceDetailListReportComponent', () => {
  let component: PartnerAdvanceDetailListReportComponent;
  let fixture: ComponentFixture<PartnerAdvanceDetailListReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerAdvanceDetailListReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerAdvanceDetailListReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
