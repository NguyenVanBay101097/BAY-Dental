import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerToathuocListComponent } from './partner-customer-toathuoc-list.component';

describe('PartnerCustomerToathuocListComponent', () => {
  let component: PartnerCustomerToathuocListComponent;
  let fixture: ComponentFixture<PartnerCustomerToathuocListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerToathuocListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerToathuocListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
