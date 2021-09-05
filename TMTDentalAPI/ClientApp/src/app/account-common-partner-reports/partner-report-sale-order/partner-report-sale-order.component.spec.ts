import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportSaleOrderComponent } from './partner-report-sale-order.component';

describe('PartnerReportSaleOrderComponent', () => {
  let component: PartnerReportSaleOrderComponent;
  let fixture: ComponentFixture<PartnerReportSaleOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportSaleOrderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportSaleOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
