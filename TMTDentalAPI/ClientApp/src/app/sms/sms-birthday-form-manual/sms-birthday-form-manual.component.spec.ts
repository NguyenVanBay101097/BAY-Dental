import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsBirthdayFormManualComponent } from './sms-birthday-form-manual.component';

describe('SmsBirthdayFormManualComponent', () => {
  let component: SmsBirthdayFormManualComponent;
  let fixture: ComponentFixture<SmsBirthdayFormManualComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsBirthdayFormManualComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsBirthdayFormManualComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
