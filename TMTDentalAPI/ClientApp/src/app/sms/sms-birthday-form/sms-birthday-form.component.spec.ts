import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsBirthdayFormComponent } from './sms-birthday-form.component';

describe('SmsBirthdayFormComponent', () => {
  let component: SmsBirthdayFormComponent;
  let fixture: ComponentFixture<SmsBirthdayFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsBirthdayFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsBirthdayFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
