import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CashBookTabPageCaBoComponent } from './cash-book-tab-page-ca-bo.component';

describe('CashBookTabPageCaBoComponent', () => {
  let component: CashBookTabPageCaBoComponent;
  let fixture: ComponentFixture<CashBookTabPageCaBoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CashBookTabPageCaBoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CashBookTabPageCaBoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
