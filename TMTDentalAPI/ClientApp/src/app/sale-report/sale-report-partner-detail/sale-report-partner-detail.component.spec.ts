import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleReportPartnerDetailComponent } from './sale-report-partner-detail.component';

describe('SaleReportPartnerDetailComponent', () => {
  let component: SaleReportPartnerDetailComponent;
  let fixture: ComponentFixture<SaleReportPartnerDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleReportPartnerDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleReportPartnerDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
