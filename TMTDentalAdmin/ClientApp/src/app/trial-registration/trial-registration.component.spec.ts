import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TrialRegistrationComponent } from './trial-registration.component';

describe('TrialRegistrationComponent', () => {
  let component: TrialRegistrationComponent;
  let fixture: ComponentFixture<TrialRegistrationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TrialRegistrationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrialRegistrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
