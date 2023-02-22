import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerTabSaleOrderListComponent } from './partner-tab-sale-order-list.component';

describe('PartnerTabSaleOrderListComponent', () => {
  let component: PartnerTabSaleOrderListComponent;
  let fixture: ComponentFixture<PartnerTabSaleOrderListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerTabSaleOrderListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerTabSaleOrderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
