import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderListServiceComponent } from './sale-order-list-service.component';

describe('SaleOrderListServiceComponent', () => {
  let component: SaleOrderListServiceComponent;
  let fixture: ComponentFixture<SaleOrderListServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderListServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderListServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
