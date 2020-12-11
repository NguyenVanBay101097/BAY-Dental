import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleReportOldNewPartnerComponent } from './sale-report-old-new-partner.component';

describe('SaleReportOldNewPartnerComponent', () => {
  let component: SaleReportOldNewPartnerComponent;
  let fixture: ComponentFixture<SaleReportOldNewPartnerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleReportOldNewPartnerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleReportOldNewPartnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
