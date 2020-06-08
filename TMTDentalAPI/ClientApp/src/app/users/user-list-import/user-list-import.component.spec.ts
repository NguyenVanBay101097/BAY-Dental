import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserListImportComponent } from './user-list-import.component';

describe('UserListImportComponent', () => {
  let component: UserListImportComponent;
  let fixture: ComponentFixture<UserListImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserListImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserListImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
