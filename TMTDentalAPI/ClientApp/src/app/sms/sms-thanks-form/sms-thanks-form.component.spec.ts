import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsThanksFormComponent } from './sms-thanks-form.component';

describe('SmsThanksFormComponent', () => {
  let component: SmsThanksFormComponent;
  let fixture: ComponentFixture<SmsThanksFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsThanksFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsThanksFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
