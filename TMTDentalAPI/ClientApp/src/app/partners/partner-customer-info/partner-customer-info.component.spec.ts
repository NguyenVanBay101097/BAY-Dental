import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerInfoComponent } from './partner-customer-info.component';

describe('PartnerCustomerInfoComponent', () => {
  let component: PartnerCustomerInfoComponent;
  let fixture: ComponentFixture<PartnerCustomerInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
