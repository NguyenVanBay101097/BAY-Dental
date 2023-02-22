import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckAddressButtonComponent } from './check-address-button.component';

describe('CheckAddressButtonComponent', () => {
  let component: CheckAddressButtonComponent;
  let fixture: ComponentFixture<CheckAddressButtonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CheckAddressButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckAddressButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
