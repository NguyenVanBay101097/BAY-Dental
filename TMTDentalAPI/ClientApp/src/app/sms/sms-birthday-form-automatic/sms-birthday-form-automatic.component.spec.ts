import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsBirthdayFormAutomaticComponent } from './sms-birthday-form-automatic.component';

describe('SmsBirthdayFormAutomaticComponent', () => {
  let component: SmsBirthdayFormAutomaticComponent;
  let fixture: ComponentFixture<SmsBirthdayFormAutomaticComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsBirthdayFormAutomaticComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsBirthdayFormAutomaticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
