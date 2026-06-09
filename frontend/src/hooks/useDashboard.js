import { useEffect, useRef, useState } from 'react'
import * as signalR from '@microsoft/signalr'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

export function useDashboard() {
  const [snapshot, setSnapshot] = useState(null)
  const [recentSales, setRecentSales] = useState([])
  const [alerts, setAlerts] = useState([])
  const [connected, setConnected] = useState(false)
  const [pulse, setPulse] = useState(false)
  const connectionRef = useRef(null)

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/hubs/dashboard`)
      .withAutomaticReconnect()
      .build()

    connection.on('SaleOccurred', (sale) => {
      setRecentSales(prev => [sale, ...prev].slice(0, 12))
      setPulse(true)
      setTimeout(() => setPulse(false), 400)
    })

    connection.on('SnapshotUpdated', (data) => {
      setSnapshot(data)
    })

    connection.on('AnomalyDetected', (alert) => {
      setAlerts(prev => [alert, ...prev].slice(0, 8))
    })

    connection.onreconnecting(() => setConnected(false))
    connection.onreconnected(() => setConnected(true))

    connection.start()
      .then(() => setConnected(true))
      .catch(console.error)

    connectionRef.current = connection

    return () => connection.stop()
  }, [])

  return { snapshot, recentSales, alerts, connected, pulse }
}
