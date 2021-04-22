import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, of, Subject } from 'rxjs';

import { ICandidate } from '../../_shared/interfaces/candidate.interface';
import { AppSettings } from '../../_shared/components/app.settings';
import { IMyStaffPostPocRequest, IMyStaffGetPocResponseRoot } from '../../_shared/interfaces/my-staff.interface';
import { IQuotationStaff } from '../../_shared/interfaces/quotation-staffs.interface';
import { IStaffAddOnPriceBookItem } from '../../_shared/interfaces/product.interface';
import { ISuiteItem } from '../../_shared/interfaces/suite.interface';

@Injectable()
export class NewQuotationService {

  candidates: ICandidate[] = [];
  suiteItems: ISuiteItem[] = [];

  myStaffNewPoc: IMyStaffPostPocRequest;
  isNewMyStaffPoc: boolean = false;

  isQuotationReadOnly = new BehaviorSubject<boolean>(false);
  selectedClientName = new BehaviorSubject<string>('');
  selectedClientCode = new BehaviorSubject<string>('');
  isNewClient = new BehaviorSubject<boolean>(true);
  selectedPriceBookGroupId = new BehaviorSubject<number>(0);
  
  companyAddOnPbis = new BehaviorSubject<IStaffAddOnPriceBookItem[]>([]);
  additionalSeatCount = new BehaviorSubject<number>(0);
  quotationId = new BehaviorSubject<number>(0);
  selectedSeatType = new Subject<any>();
  selectedSuites = new Subject<ISuiteItem[]>();
  clientPoc = new Subject<IMyStaffGetPocResponseRoot>();

  newQuotationStaffs = new BehaviorSubject<IQuotationStaff[]>([]);
  activeClientStaffs = new Subject<IClientActiveStaffDetailsResponse[]>();
  selectedStaffsAddOns = new BehaviorSubject<IStaffAddOnPriceBookItem[]>([]);
  selectedStaffsForAddOn = new BehaviorSubject<IQuotationStaff[]>([]);
  selectedStaffsToUpdate = new Subject<any>();
  seatSelectedStaffs = new BehaviorSubject<any>([]);

  constructor(private http: HttpClient, private appSettings: AppSettings) { }

  searchCandidatesById(inputString: string) {
    if (this.candidates.length > 10) {
      const filteredCandidates = this.candidates.filter(candidate => candidate.id.toString().startsWith(inputString))
      if (filteredCandidates.length > 0) return of(filteredCandidates);
    }

    return this.http.get<ICandidate[]>(this.appSettings.env.API_BASE_URL + `/api/Quotation/GetAllCandidates/${inputString}`).pipe(
      map(response => {
        this.candidates = response;

        return response;
      })
    )
  }

  getPocByClientCode(clientCode: string, clientName: string) {

    let params = new HttpParams();
    params = params.append('client_code', clientCode);
    params = params.append('client_name', clientName);

    return this.http.get<IMyStaffGetPocResponseRoot>
    (this.appSettings.env.API_BASE_URL + `/api/Quotation/GetPointOfContract`, { params }).pipe(
      map(response => {
        this.clientPoc.next(response);
        return response;
      })
    )
  }

  postPocToMyStaff() {
    return this.http.post(this.appSettings.env.API_BASE_URL + `/api/Quotation/PostPointOfContract`, this.myStaffNewPoc)
    .pipe(
      map(response => {
        return response;
      })
    )
  }

  getClientActiveStaff(clientCode: string) {

    let params = new HttpParams();
    params = params.append('clientCode', clientCode);

    return this.http.get<IClientActiveStaffDetailsResponse[]>
    (this.appSettings.env.API_BASE_URL + `/api/QuotationV2/GetStaffContractByClientCode`, { params })
    .pipe(
      map(response => {

        this.activeClientStaffs.next(response);
        return response;
      })
    )
  }

  getCompanyPbis(priceBookId: number, excludeAddOn = 'StaffAddOn') {
    let params = new HttpParams();
    params = params.append('priceBookId', priceBookId.toString());

    if (excludeAddOn) params = params.append('excludeAddOn', excludeAddOn);

    return this.http.get<IStaffAddOnPriceBookItem[]>
      (this.appSettings.env.API_BASE_URL + `/api/ProductV2/GetPriceBookItemsByGroup`, { params })
      .pipe(
        map(response => {

          this.companyAddOnPbis.next(response);
          return response;
        })
      )
  }

  getStaffPbis(priceBookId: number, excludeAddOn = 'CompanyAddOn') {
    let params = new HttpParams();
    params = params.append('priceBookId', priceBookId.toString());

    if (excludeAddOn) params = params.append('excludeAddOn', excludeAddOn);

    return this.http.get<IStaffAddOnPriceBookItem[]>
      (this.appSettings.env.API_BASE_URL + `/api/ProductV2/GetPriceBookItemsByGroup`, { params })
      .pipe(
        map(response => {

          this.selectedStaffsAddOns.next(response);
          return response;
        })
      )
  }

  getSuiteItems(suiteId: number) {

    if(this.suiteItems.length > 0 && suiteId === this.suiteItems[0].suiteId) return of(this.suiteItems);

    let params = new HttpParams();
    params = params.append('suiteId', suiteId.toString());

    return this.http.get<ISuiteItem[]>
      (this.appSettings.env.API_BASE_URL + `/api/SuiteV2/GetSuiteItemsBySuiteId?suiteId=1`, { params })
      .pipe(
        map(response => {

          this.suiteItems = response;
          return response;
        })
      )
  }
}
