import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountAccountListComponent } from './account-account-list.component';

describe('AccountAccountListComponent', () => {
  let component: AccountAccountListComponent;
  let fixture: ComponentFixture<AccountAccountListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountAccountListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountAccountListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
