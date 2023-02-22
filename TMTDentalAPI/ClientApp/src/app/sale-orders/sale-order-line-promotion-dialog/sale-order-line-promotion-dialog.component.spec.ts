import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLinePromotionDialogComponent } from './sale-order-line-promotion-dialog.component';

describe('SaleOrderLinePromotionDialogComponent', () => {
  let component: SaleOrderLinePromotionDialogComponent;
  let fixture: ComponentFixture<SaleOrderLinePromotionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLinePromotionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLinePromotionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
