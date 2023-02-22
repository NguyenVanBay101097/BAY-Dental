import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerDebitDetailListReportComponent } from './partner-debit-detail-list-report.component';

describe('PartnerDebitDetailListReportComponent', () => {
  let component: PartnerDebitDetailListReportComponent;
  let fixture: ComponentFixture<PartnerDebitDetailListReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerDebitDetailListReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerDebitDetailListReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
