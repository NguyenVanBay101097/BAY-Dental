import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookMassMessagingCreateUpdateComponent } from './facebook-mass-messaging-create-update.component';

describe('FacebookMassMessagingCreateUpdateComponent', () => {
  let component: FacebookMassMessagingCreateUpdateComponent;
  let fixture: ComponentFixture<FacebookMassMessagingCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookMassMessagingCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookMassMessagingCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
