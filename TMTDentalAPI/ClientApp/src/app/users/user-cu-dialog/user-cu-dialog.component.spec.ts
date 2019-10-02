import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserCuDialogComponent } from './user-cu-dialog.component';

describe('UserCuDialogComponent', () => {
  let component: UserCuDialogComponent;
  let fixture: ComponentFixture<UserCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
