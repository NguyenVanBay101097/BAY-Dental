import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineLaboOrdersDialogComponent } from './sale-order-line-labo-orders-dialog.component';

describe('SaleOrderLineLaboOrdersDialogComponent', () => {
  let component: SaleOrderLineLaboOrdersDialogComponent;
  let fixture: ComponentFixture<SaleOrderLineLaboOrdersDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineLaboOrdersDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineLaboOrdersDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
