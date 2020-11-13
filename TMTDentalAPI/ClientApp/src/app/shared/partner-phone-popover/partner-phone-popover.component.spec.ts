import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerPhonePopoverComponent } from './partner-phone-popover.component';

describe('PartnerPhonePopoverComponent', () => {
  let component: PartnerPhonePopoverComponent;
  let fixture: ComponentFixture<PartnerPhonePopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerPhonePopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerPhonePopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
