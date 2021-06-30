import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerDebitListReportComponent } from './partner-debit-list-report.component';

describe('PartnerDebitListReportComponent', () => {
  let component: PartnerDebitListReportComponent;
  let fixture: ComponentFixture<PartnerDebitListReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerDebitListReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerDebitListReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
