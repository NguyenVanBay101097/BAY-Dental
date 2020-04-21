import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookMassMessagingsComponent } from './facebook-mass-messagings.component';

describe('FacebookMassMessagingsComponent', () => {
  let component: FacebookMassMessagingsComponent;
  let fixture: ComponentFixture<FacebookMassMessagingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookMassMessagingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookMassMessagingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
