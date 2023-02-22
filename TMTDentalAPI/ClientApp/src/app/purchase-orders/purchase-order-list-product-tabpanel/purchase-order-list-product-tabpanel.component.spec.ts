import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOrderListProductTabpanelComponent } from './purchase-order-list-product-tabpanel.component';

describe('PurchaseOrderListProductTabpanelComponent', () => {
  let component: PurchaseOrderListProductTabpanelComponent;
  let fixture: ComponentFixture<PurchaseOrderListProductTabpanelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseOrderListProductTabpanelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseOrderListProductTabpanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
