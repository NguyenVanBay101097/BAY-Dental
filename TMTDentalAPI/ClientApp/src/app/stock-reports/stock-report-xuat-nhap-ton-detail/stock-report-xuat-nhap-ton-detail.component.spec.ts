import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockReportXuatNhapTonDetailComponent } from './stock-report-xuat-nhap-ton-detail.component';

describe('StockReportXuatNhapTonDetailComponent', () => {
  let component: StockReportXuatNhapTonDetailComponent;
  let fixture: ComponentFixture<StockReportXuatNhapTonDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockReportXuatNhapTonDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockReportXuatNhapTonDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
