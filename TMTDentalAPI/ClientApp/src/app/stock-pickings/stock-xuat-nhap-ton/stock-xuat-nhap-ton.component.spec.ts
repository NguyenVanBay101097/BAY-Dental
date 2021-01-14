import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockXuatNhapTonComponent } from './stock-xuat-nhap-ton.component';

describe('StockXuatNhapTonComponent', () => {
  let component: StockXuatNhapTonComponent;
  let fixture: ComponentFixture<StockXuatNhapTonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockXuatNhapTonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockXuatNhapTonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
