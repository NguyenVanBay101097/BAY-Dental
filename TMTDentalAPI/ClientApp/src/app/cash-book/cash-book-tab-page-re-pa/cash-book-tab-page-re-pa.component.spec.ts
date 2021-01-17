import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CashBookTabPageRePaComponent } from './cash-book-tab-page-re-pa.component';

describe('CashBookTabPageRePaComponent', () => {
  let component: CashBookTabPageRePaComponent;
  let fixture: ComponentFixture<CashBookTabPageRePaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CashBookTabPageRePaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CashBookTabPageRePaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
