import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerProfilePrintComponent } from './partner-profile-print.component';

describe('PartnerProfilePrintComponent', () => {
  let component: PartnerProfilePrintComponent;
  let fixture: ComponentFixture<PartnerProfilePrintComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerProfilePrintComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerProfilePrintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
