import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleReportPartnerComponent } from './sale-report-partner.component';

describe('SaleReportPartnerComponent', () => {
  let component: SaleReportPartnerComponent;
  let fixture: ComponentFixture<SaleReportPartnerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleReportPartnerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleReportPartnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
