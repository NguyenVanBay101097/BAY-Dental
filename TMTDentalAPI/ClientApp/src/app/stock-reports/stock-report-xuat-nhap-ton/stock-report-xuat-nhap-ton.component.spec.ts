import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockReportXuatNhapTonComponent } from './stock-report-xuat-nhap-ton.component';

describe('StockReportXuatNhapTonComponent', () => {
  let component: StockReportXuatNhapTonComponent;
  let fixture: ComponentFixture<StockReportXuatNhapTonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockReportXuatNhapTonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockReportXuatNhapTonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
