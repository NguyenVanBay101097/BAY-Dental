import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerListDetailComponent } from './partner-customer-list-detail.component';

describe('PartnerCustomerListDetailComponent', () => {
  let component: PartnerCustomerListDetailComponent;
  let fixture: ComponentFixture<PartnerCustomerListDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerListDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerListDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
