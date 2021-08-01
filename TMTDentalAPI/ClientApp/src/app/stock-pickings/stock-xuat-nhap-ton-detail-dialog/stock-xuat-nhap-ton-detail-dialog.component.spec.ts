import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockXuatNhapTonDetailDialogComponent } from './stock-xuat-nhap-ton-detail-dialog.component';

describe('StockXuatNhapTonDetailDialogComponent', () => {
  let component: StockXuatNhapTonDetailDialogComponent;
  let fixture: ComponentFixture<StockXuatNhapTonDetailDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockXuatNhapTonDetailDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockXuatNhapTonDetailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
