import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerProductToaThuocListComponent } from './partner-customer-product-toa-thuoc-list.component';

describe('PartnerCustomerProductToaThuocListComponent', () => {
  let component: PartnerCustomerProductToaThuocListComponent;
  let fixture: ComponentFixture<PartnerCustomerProductToaThuocListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerProductToaThuocListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerProductToaThuocListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
