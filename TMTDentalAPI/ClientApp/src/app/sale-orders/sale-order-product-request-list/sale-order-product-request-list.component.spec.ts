import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderProductRequestListComponent } from './sale-order-product-request-list.component';

describe('SaleOrderProductRequestListComponent', () => {
  let component: SaleOrderProductRequestListComponent;
  let fixture: ComponentFixture<SaleOrderProductRequestListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderProductRequestListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderProductRequestListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
