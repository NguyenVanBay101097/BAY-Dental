import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountAccountFormComponent } from './account-account-form.component';

describe('AccountAccountFormComponent', () => {
  let component: AccountAccountFormComponent;
  let fixture: ComponentFixture<AccountAccountFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountAccountFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountAccountFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
