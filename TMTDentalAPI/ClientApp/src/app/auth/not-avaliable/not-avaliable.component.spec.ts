import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NotAvaliableComponent } from './not-avaliable.component';

describe('NotAvaliableComponent', () => {
  let component: NotAvaliableComponent;
  let fixture: ComponentFixture<NotAvaliableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NotAvaliableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotAvaliableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
