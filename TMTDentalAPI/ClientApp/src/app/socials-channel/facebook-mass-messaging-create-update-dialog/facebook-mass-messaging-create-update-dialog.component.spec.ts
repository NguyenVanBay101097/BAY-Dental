import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookMassMessagingCreateUpdateDialogComponent } from './facebook-mass-messaging-create-update-dialog.component';

describe('FacebookMassMessagingCreateUpdateDialogComponent', () => {
  let component: FacebookMassMessagingCreateUpdateDialogComponent;
  let fixture: ComponentFixture<FacebookMassMessagingCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookMassMessagingCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookMassMessagingCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
