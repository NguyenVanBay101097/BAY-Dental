import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerQuotationsComponent } from './partner-customer-quotations.component';

describe('PartnerCustomerQuotationsComponent', () => {
  let component: PartnerCustomerQuotationsComponent;
  let fixture: ComponentFixture<PartnerCustomerQuotationsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerQuotationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerQuotationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
