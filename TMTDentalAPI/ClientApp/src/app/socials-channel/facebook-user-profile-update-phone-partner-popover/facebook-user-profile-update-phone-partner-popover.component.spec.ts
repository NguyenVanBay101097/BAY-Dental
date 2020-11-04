import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookUserProfileUpdatePhonePartnerPopoverComponent } from './facebook-user-profile-update-phone-partner-popover.component';

describe('FacebookUserProfileUpdatePhonePartnerPopoverComponent', () => {
  let component: FacebookUserProfileUpdatePhonePartnerPopoverComponent;
  let fixture: ComponentFixture<FacebookUserProfileUpdatePhonePartnerPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookUserProfileUpdatePhonePartnerPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookUserProfileUpdatePhonePartnerPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
