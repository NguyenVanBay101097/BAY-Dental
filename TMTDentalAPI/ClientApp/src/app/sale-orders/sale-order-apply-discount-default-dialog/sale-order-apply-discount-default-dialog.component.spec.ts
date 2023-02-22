import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderApplyDiscountDefaultDialogComponent } from './sale-order-apply-discount-default-dialog.component';

describe('SaleOrderApplyDiscountDefaultDialogComponent', () => {
  let component: SaleOrderApplyDiscountDefaultDialogComponent;
  let fixture: ComponentFixture<SaleOrderApplyDiscountDefaultDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderApplyDiscountDefaultDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderApplyDiscountDefaultDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
