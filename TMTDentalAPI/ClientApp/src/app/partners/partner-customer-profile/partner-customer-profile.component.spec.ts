import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerProfileComponent } from './partner-customer-profile.component';

describe('PartnerCustomerProfileComponent', () => {
  let component: PartnerCustomerProfileComponent;
  let fixture: ComponentFixture<PartnerCustomerProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
