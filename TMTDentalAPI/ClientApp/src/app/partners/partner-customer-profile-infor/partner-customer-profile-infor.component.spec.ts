import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerProfileInforComponent } from './partner-customer-profile-infor.component';

describe('PartnerCustomerProfileInforComponent', () => {
  let component: PartnerCustomerProfileInforComponent;
  let fixture: ComponentFixture<PartnerCustomerProfileInforComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerProfileInforComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerProfileInforComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
