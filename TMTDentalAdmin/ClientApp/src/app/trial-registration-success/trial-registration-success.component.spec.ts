import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TrialRegistrationSuccessComponent } from './trial-registration-success.component';

describe('TrialRegistrationSuccessComponent', () => {
  let component: TrialRegistrationSuccessComponent;
  let fixture: ComponentFixture<TrialRegistrationSuccessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TrialRegistrationSuccessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrialRegistrationSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
